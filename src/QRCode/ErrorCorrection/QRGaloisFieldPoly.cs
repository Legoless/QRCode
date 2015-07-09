// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrGaloisFieldPoly.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The  galois field polynomial.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.ErrorCorrection
{
    using System;
    using System.Text;

    using NLog;

    /// <summary>
    /// The galois field polynomial.
    /// </summary>
    internal class QrGaloisFieldPoly
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The _coefficients.
        /// </summary>
        private readonly int[] coefficients;

        /// <summary>
        /// The _field.
        /// </summary>
        private readonly QrGaloisField field;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGaloisFieldPoly"/> class.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="galoisCoefficients">
        /// The coefficients.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Argument exception if empty array of coefficients is provided
        /// </exception>
        public QrGaloisFieldPoly(QrGaloisField field, int[] galoisCoefficients)
        {
            if (galoisCoefficients.Length == 0)
            {
                throw new ArgumentException();
            }

            this.field = field;

            int coefficientsLength = galoisCoefficients.Length;

            if (coefficientsLength > 1 && galoisCoefficients[0] == 0)
            {
                // Leading term must be non-zero for anything except the constant polynomial "0"
                int firstNonZero = 1;

                while (firstNonZero < coefficientsLength && galoisCoefficients[firstNonZero] == 0)
                {
                    firstNonZero++;
                }

                if (firstNonZero == coefficientsLength)
                {
                    this.coefficients = field.Zero.coefficients;
                }
                else
                {
                    this.coefficients = new int[coefficientsLength - firstNonZero];

                    Array.Copy(galoisCoefficients, firstNonZero, this.coefficients, 0, this.coefficients.Length);
                }
            }
            else
            {
                this.coefficients = galoisCoefficients;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add or subtract.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="QrGaloisFieldPoly"/>.
        /// </returns>
        public QrGaloisFieldPoly AddOrSubtract(QrGaloisFieldPoly other)
        {
            if (this.IsZero())
            {
                return other;
            }

            if (other.IsZero())
            {
                return this;
            }

            int[] smallerCoefficients = this.coefficients;
            int[] largerCoefficients = other.coefficients;

            if (smallerCoefficients.Length > largerCoefficients.Length)
            {
                int[] temp = smallerCoefficients;
                smallerCoefficients = largerCoefficients;
                largerCoefficients = temp;
            }

            int[] sumDiff = new int[largerCoefficients.Length];
            int lengthDiff = largerCoefficients.Length - smallerCoefficients.Length;

            // Copy high-order terms only found in higher-power polynomial's coefficients
            Array.Copy(largerCoefficients, 0, sumDiff, 0, lengthDiff);

            for (int i = lengthDiff; i < largerCoefficients.Length; i++)
            {
                sumDiff[i] = QrGaloisField.AddOrSubtract(smallerCoefficients[i - lengthDiff], largerCoefficients[i]);
            }

            return new QrGaloisFieldPoly(this.field, sumDiff);
        }

        /// <summary>
        /// The divide.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="QrGaloisFieldPoly" /> array.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Exception if division by zero.
        /// </exception>
        public QrGaloisFieldPoly[] Divide(QrGaloisFieldPoly other)
        {
            if (other.IsZero())
            {
                throw new ArgumentException("Divide by 0");
            }

            QrGaloisFieldPoly quotient = this.field.Zero;
            QrGaloisFieldPoly remainder = this;

            int denominatorLeadingTerm = other.GetCoefficient(other.GetPower());
            int inverseDenominatorLeadingTerm = this.field.Inverse(denominatorLeadingTerm);

            while ((remainder.GetPower() >= other.GetPower()) && !remainder.IsZero())
            {
                int powerDifference = remainder.GetPower() - other.GetPower();
                int scale = this.field.Multiply(
                    remainder.GetCoefficient(remainder.GetPower()), inverseDenominatorLeadingTerm);

                QrGaloisFieldPoly term = other.MultiplyByMonomial(powerDifference, scale);
                QrGaloisFieldPoly iterationQuotient = this.field.BuildMonomial(powerDifference, scale);

                quotient = quotient.AddOrSubtract(iterationQuotient);
                remainder = remainder.AddOrSubtract(term);
            }

            return new[] { quotient, remainder };
        }

        /// <summary>
        /// The evaluate at.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int EvaluateAt(int a)
        {
            // Just return the x^0 coefficient
            if (a == 0)
            {
                return this.GetCoefficient(0);
            }

            int size = this.coefficients.Length;

            if (a == 1)
            {
                // Just the sum of the coefficients
                int txt = 0;

                for (int i = 0; i < size; i++)
                {
                    txt = QrGaloisField.AddOrSubtract(txt, this.coefficients[i]);
                }

                return txt;
            }

            int res = this.coefficients[0];

            for (int i = 1; i < size; i++)
            {
                res = QrGaloisField.AddOrSubtract(this.field.Multiply(a, res), this.coefficients[i]);
            }

            return res;
        }

        /// <summary>
        /// The get coefficient.
        /// </summary>
        /// <param name="power">
        /// The power.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetCoefficient(int power)
        {
            return this.coefficients[this.coefficients.Length - 1 - power];
        }

        /// <summary>
        /// The get coefficients.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int[] GetCoefficients()
        {
            return this.coefficients;
        }

        /// <summary>
        /// The get power.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetPower()
        {
            return this.coefficients.Length - 1;
        }

        /// <summary>
        /// The is zero.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsZero()
        {
            return this.coefficients[0] == 0;
        }

        /// <summary>
        /// The multiply.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="QrGaloisFieldPoly"/>.
        /// </returns>
        public QrGaloisFieldPoly Multiply(QrGaloisFieldPoly other)
        {
            if (this.IsZero() || other.IsZero())
            {
                return this.field.Zero;
            }

            int[] galoisACoefficients = this.coefficients;
            int galoisALength = galoisACoefficients.Length;

            int[] galoisBCoefficients = other.coefficients;

            int galoisBLength = galoisBCoefficients.Length;
            int[] product = new int[galoisALength + galoisBLength - 1];

            for (int i = 0; i < galoisALength; i++)
            {
                int galoisACoeff = galoisACoefficients[i];

                for (int j = 0; j < galoisBLength; j++)
                {
                    product[i + j] = QrGaloisField.AddOrSubtract(product[i + j], this.field.Multiply(galoisACoeff, galoisBCoefficients[j]));
                }
            }

            return new QrGaloisFieldPoly(this.field, product);
        }

        /// <summary>
        /// The multiply.
        /// </summary>
        /// <param name="scalar">
        /// The scalar.
        /// </param>
        /// <returns>
        /// The <see cref="QrGaloisFieldPoly"/>.
        /// </returns>
        public QrGaloisFieldPoly Multiply(int scalar)
        {
            if (scalar == 0)
            {
                return this.field.Zero;
            }

            if (scalar == 1)
            {
                return this;
            }

            int size = this.coefficients.Length;
            int[] product = new int[size];

            for (int i = 0; i < size; i++)
            {
                product[i] = this.field.Multiply(this.coefficients[i], scalar);
            }

            return new QrGaloisFieldPoly(this.field, product);
        }

        /// <summary>
        /// The multiply by monomial.
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
        /// <exception cref="ArgumentException">
        /// Exception if power is less than zero.
        /// </exception>
        public QrGaloisFieldPoly MultiplyByMonomial(int power, int coefficient)
        {
            if (power < 0)
            {
                throw new ArgumentException();
            }

            if (coefficient == 0)
            {
                return this.field.Zero;
            }

            int size = this.coefficients.Length;
            int[] product = new int[size + power];

            for (int i = 0; i < size; i++)
            {
                product[i] = this.field.Multiply(this.coefficients[i], coefficient);
            }

            return new QrGaloisFieldPoly(this.field, product);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder txt = new StringBuilder(8 * this.GetPower());

            for (int power = this.GetPower(); power >= 0; power--)
            {
                int coefficient = this.GetCoefficient(power);

                if (coefficient != 0)
                {
                    if (coefficient < 0)
                    {
                        txt.Append(" - ");
                        coefficient = -coefficient;
                    }
                    else
                    {
                        if (txt.Length > 0)
                        {
                            txt.Append(" + ");
                        }
                    }

                    if ((power == 0) || (coefficient != 1))
                    {
                        int alphaPower = this.field.GetLog(coefficient);

                        if (alphaPower == 0)
                        {
                            txt.Append('1');
                        }
                        else if (alphaPower == 1)
                        {
                            txt.Append('a');
                        }
                        else
                        {
                            txt.Append("a^");
                            txt.Append(alphaPower);
                        }
                    }

                    if (power != 0)
                    {
                        if (power == 1)
                        {
                            txt.Append('x');
                        }
                        else
                        {
                            txt.Append("x^");
                            txt.Append(power);
                        }
                    }
                }
            }

            return txt.ToString();
        }

        #endregion
    }
}