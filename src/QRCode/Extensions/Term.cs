// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Term.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The term.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Extensions
{
    using System.Globalization;

    /// <summary>
    /// The term.
    /// </summary>
    public class Term
    {
        #region Fields

        /// <summary>
        /// Private field to hold the Power Value.
        /// </summary>
        private int power;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Term"/> class. 
        /// Simple Constructor which Create a new Instance of Term Class
        /// With 2 parameters
        /// </summary>
        /// <param name="power">
        /// Power of term
        /// </param>
        /// <param name="coefficient">
        /// Coefficient of term
        /// </param>
        public Term(int power, int coefficient)
        {
            this.Power = power;
            this.Coefficient = coefficient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Term"/> class. 
        /// Constructor Overload which Create a new Instance of Term Class
        /// With a simple string and try to read the Power and Coefficient
        /// by identifying the input string
        /// </summary>
        /// <param name="termExpression">
        /// Expression as a string
        /// </param>
        public Term(string termExpression)
        {
            if (termExpression.Length > 0)
            {
                if (termExpression.IndexOf("x^", System.StringComparison.Ordinal) > -1)
                {
                    string coefficientString = termExpression.Substring(
                        0, termExpression.IndexOf("x^", System.StringComparison.Ordinal));
                    int indexofX = termExpression.IndexOf("x^", System.StringComparison.Ordinal);
                    string powerString = termExpression.Substring(
                        indexofX + 2, (termExpression.Length - 1) - (indexofX + 1));
                    if (coefficientString == "-")
                    {
                        this.Coefficient = -1;
                    }
                    else if (coefficientString == "+" | coefficientString == string.Empty)
                    {
                        this.Coefficient = 1;
                    }
                    else
                    {
                        this.Coefficient = int.Parse(coefficientString);
                    }

                    this.Power = int.Parse(powerString);
                }
                else if (termExpression.IndexOf("x", System.StringComparison.Ordinal) > -1)
                {
                    this.Power = 1;
                    string coefficientString = termExpression.Substring(
                        0, termExpression.IndexOf("x", System.StringComparison.Ordinal));
                    if (coefficientString == "-")
                    {
                        this.Coefficient = -1;
                    }
                    else if (coefficientString == "+" | coefficientString == string.Empty)
                    {
                        this.Coefficient = 1;
                    }
                    else
                    {
                        this.Coefficient = int.Parse(coefficientString);
                    }
                }
                else
                {
                    this.Power = 0;
                    this.Coefficient = int.Parse(termExpression);
                }
            }
            else
            {
                this.Power = 0;
                this.Coefficient = 0;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the coefficient property
        /// </summary>
        public int Coefficient { get; set; }

        /// <summary>
        /// Gets or sets the power property
        /// Notice: Set Method Check if the value is Negative and Make it Positive.
        /// </summary>
        public int Power
        {
            get
            {
                return this.power;
            }

            set
            {
                if (value < 0)
                {
                    this.power = value * -1;
                }
                else
                {
                    this.power = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// This Override will push the Term in a String Form to the output.
        /// ToString() method will write the String Format of the Term Like: 3x^2
        /// Which means [Coefficient]x^[Power].
        /// This Method Also check if it's needed to have x^,x,- or + in the pattern.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Coefficient >= 0)
            {
                result += " + ";
            }
            else
            {
                result += " - ";
            }

            if (this.Power == 0)
            {
                result +=
                    (this.Coefficient < 0 ? this.Coefficient * -1 : this.Coefficient).ToString(
                        CultureInfo.InvariantCulture);
            }
            else if (this.Power == 1)
            {
                if (this.Coefficient > 1 | this.Coefficient < -1)
                {
                    result += string.Format("{0} x ", (this.Coefficient < 0 ? this.Coefficient * -1 : this.Coefficient).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    result += "x";
                }
            }
            else if (this.Coefficient > 1 | this.Coefficient < -1)
            {
                result += string.Format("{0}x ^ {1}", (this.Coefficient < 0 ? this.Coefficient * -1 : this.Coefficient).ToString(CultureInfo.InvariantCulture), this.Power.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                result += string.Format("x ^ {0}", this.Power.ToString(CultureInfo.InvariantCulture));
            }

            return result;
        }

        #endregion
    }
}